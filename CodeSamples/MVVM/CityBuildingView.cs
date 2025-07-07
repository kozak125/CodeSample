using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Voidwalker.Systems.Items;
using Voidwalker.Systems.Rewards;
using Voidwalker.Systems.Saving;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

namespace Voidwalker.City
{
    public abstract class CityBuildingView<TViewModel, TModel> : BuildingView, IPersistentObjectState
        where TViewModel : CityBuildingViewModel<TModel>
        where TModel : CityBuildingModel
    {
        [SerializeField]
        protected float fadeTime;
        [SerializeField, Required]
        protected Image eventBackground;
        [SerializeField]
        protected Image bookBackground;
        [SerializeField, Required]
        protected Image eventImage;
        [SerializeField, Required, FoldoutGroup("Title section")]
        protected TMP_Text titleText;
        [SerializeField, Required, FoldoutGroup("Title section")]
        protected Image titleDivider;
        [SerializeField, Required]
        protected TMP_Text descriptionText;
        [SerializeField, Required, FoldoutGroup("Option section")]
        protected Button exploreOptionButton;
        [SerializeField, Required, FoldoutGroup("Option section")]
        protected Button buildingSpecificOptionButton;
        [SerializeField, Required, FoldoutGroup("Option section"), ListDrawerSettings(ShowIndexLabels = true)]
        protected Image[] optionDividers;
        [SerializeField, Required]
        protected Button closeViewButton;
        [SerializeField]
        protected Button previousViewButton;
        [SerializeField, Required]
        protected Button interactionBlockingButton;
        [SerializeField, Required]
        protected StoryRewardData exploreStory;
        [SerializeField, RequiredIn(PrefabKind.InstanceInScene | PrefabKind.NonPrefabInstance)]
        protected GeneralResourcesHUD generalResourcesHUD;
        [SerializeField, RequiredIn(PrefabKind.InstanceInScene | PrefabKind.NonPrefabInstance)]
        protected RewardsManager rewardsManager;
        [SerializeField, Required, InlineEditor]
        protected TModel model;
        [field: SerializeField, ReadOnly]
        public string GUID { get; set; }

        protected TViewModel viewModel;
        protected TMP_Text exploreOptionButtonText;
        protected TMP_Text buildingSpecificOptionButtonText;
        protected TMP_Text closeViewButtonText;
        protected TMP_Text previousViewButtonText;
        protected List<MaskableGraphic> mainPageGraphics;
        protected List<MaskableGraphic> extraPageGraphics;
        protected Sequence fadingSequence;
        protected List<bool> internalStates;

        protected const float VIEW_SWITCH_DELAY = 0.1f;

        public virtual void SaveData(ref LevelSpecificGameData gameData)
        {
            if (gameData.ObjectToInternalStatesPair.ContainsKey(GUID))
            {
                gameData.ObjectToInternalStatesPair.Remove(GUID);
            }

            SetupInternalStates();
            SerializableDictionaryValueList internalStatesValues = new(internalStates);

            gameData.ObjectToInternalStatesPair.Add(GUID, internalStatesValues);
        }

        public virtual void LoadData(LevelSpecificGameData gameData)
        {
            if (gameData.ObjectToInternalStatesPair.ContainsKey(GUID))
            {
                exploreOptionButton.interactable = gameData.ObjectToInternalStatesPair[GUID].Values[0];
            }
        }

        public void ResetGUID()
        {
            GUID = null;
        }

        public void OnExitButtonClicked()
        {
            viewModel.OnExitButtonClicked();
        }

        public void OnBackButtonClicked()
        {
            viewModel.OnBackButtonClicked();
        }

        public void OnExploreButtonClicked()
        {
            viewModel.OnExploreButtonClicked(exploreStory);
        }

        public override void OnOpenViewButtonClicked()
        {
            viewModel.OnOpenViewButtonClicked();
        }

        protected virtual void Start()
        {
            exploreOptionButtonText = exploreOptionButton.GetComponentInChildren<TMP_Text>(true);
            buildingSpecificOptionButtonText = buildingSpecificOptionButton.GetComponentInChildren<TMP_Text>(true);
            closeViewButtonText = closeViewButton.GetComponentInChildren<TMP_Text>(true);

            if (previousViewButton != null)
            {
                previousViewButtonText = previousViewButton.GetComponentInChildren<TMP_Text>(true);
            }

            SetupMainPageGraphics();
            SetupExtraPageGraphics();

            viewModel.ChangingViewToMainPage += OnChangingViewToMainPage;
            viewModel.ChangingViewToExtraPage += OnChangingViewToExtraPage;
            viewModel.OpeningView += OnOpeningView;
            viewModel.ClosingView += OnClosingView;
            viewModel.ExploreButtonDisabling += OnExploreButtonDisabling;
        }

        protected abstract void OnChangingViewToExtraPage();

        protected abstract void OnChangingViewToMainPage();

        protected abstract void SetupViewWithMainPageData();

        protected void SetupFadeOutCurrentPage(List<MaskableGraphic> pageContents)
        {
            TryKillFadingSequence();
            interactionBlockingButton.gameObject.SetActive(true);
            AddFadeOutSequence(pageContents.ToArray());
        }

        protected void SetupFadeInNextPage(List<MaskableGraphic> pageContents)
        {
            AddFadeInSequence(pageContents.ToArray());
            fadingSequence.AppendCallback(() => interactionBlockingButton.gameObject.SetActive(false));
        }

        protected virtual void OnClosingView()
        {
            EventSystem.current.SetSelectedGameObject(null);
            TryKillFadingSequence();

            foreach (var graphic in mainPageGraphics)
            {
                ToggleElementState(false, graphic);
            }

            ToggleElementState(false, bookBackground);
            eventBackground.gameObject.SetActive(false);
            exploreOptionButton.gameObject.SetActive(false);
            buildingSpecificOptionButton.gameObject.SetActive(false);
            generalResourcesHUD.gameObject.SetActive(true);
        }

        protected virtual void OnOpeningView()
        {
            EventSystem.current.SetSelectedGameObject(null);
            generalResourcesHUD.gameObject.SetActive(false);
            SetupViewWithMainPageData();
            TryKillFadingSequence();
            interactionBlockingButton.gameObject.SetActive(true);
            eventBackground.gameObject.SetActive(true);
            AddFadeInSequence(bookBackground);
            AddFadeInSequence(mainPageGraphics.ToArray());
            fadingSequence.
                JoinCallback(() => exploreOptionButton.gameObject.SetActive(true)).
                JoinCallback(() => buildingSpecificOptionButton.gameObject.SetActive(true)).
                AppendCallback(() => interactionBlockingButton.gameObject.SetActive(false));
            fadingSequence.Play();
        }

        protected void OnExploreButtonDisabling()
        {
            exploreOptionButton.interactable = false;
        }

        protected void AddFadeInSequence(params MaskableGraphic[] viewElements)
        {
            foreach (var element in viewElements)
            {
                fadingSequence.
                    JoinCallback(() => element.gameObject.SetActive(true)).
                    Join(element.DOFade(1, fadeTime).OnKill(() => ToggleElementState(true, element)));
            }
        }

        protected void AddFadeOutSequence(params MaskableGraphic[] viewElements)
        {
            foreach (var element in viewElements)
            {
                Sequence singleElementFadingSequence = DOTween.Sequence();
                singleElementFadingSequence.
                    Append(element.DOFade(0, fadeTime).OnKill(() => ToggleElementState(false, element))).
                    AppendCallback(() => element.gameObject.SetActive(false));

                fadingSequence.Join(singleElementFadingSequence);
            }
        }

        protected void TryKillFadingSequence()
        {
            if (fadingSequence.IsActive() && fadingSequence.IsPlaying())
            {
                fadingSequence.Kill();
            }

            fadingSequence = DOTween.Sequence();
        }

        protected void ToggleElementState(bool isOn, MaskableGraphic element)
        {
            float opacity = isOn ? 1 : 0;

            element.gameObject.SetActive(isOn);
            SetViewElementOpacity(opacity, element);
        }

        protected void SetViewElementOpacity(float opacity, MaskableGraphic element)
        {
            Color elementOpacity = element.color;
            elementOpacity.a = opacity;
            element.color = elementOpacity;
        }

        protected virtual void SetupMainPageGraphics()
        {
            mainPageGraphics = new(20)
            {
                titleText,
                titleDivider,
                eventImage,
                descriptionText,
                exploreOptionButtonText,
                buildingSpecificOptionButtonText,
                closeViewButton.image,
                closeViewButtonText
            };

            mainPageGraphics.AddRange(optionDividers);
        }

        protected virtual void SetupExtraPageGraphics()
        {
            extraPageGraphics = new(20)
            {
                titleDivider,
                previousViewButton.image,
                previousViewButtonText
            };
        }

        protected virtual void SetupInternalStates()
        {
            internalStates = new()
            {
                exploreOptionButton.interactable
            };
        }

        protected virtual void OnDestroy()
        {
            mainPageGraphics.Clear();
            extraPageGraphics?.Clear();
            internalStates?.Clear();
            viewModel?.Dispose();

            viewModel = null;
            exploreOptionButtonText = null;
            buildingSpecificOptionButtonText = null;
            closeViewButtonText = null;
            previousViewButtonText = null;
        }
    }
}
