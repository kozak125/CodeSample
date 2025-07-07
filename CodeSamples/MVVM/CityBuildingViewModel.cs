using System;
using Voidwalker.Systems.Rewards;
using Voidwalker.Systems.Saving;

namespace Voidwalker.City
{
    public abstract class CityBuildingViewModel<TModel> : BuildingViewModel where TModel : CityBuildingModel
    {
        public event Action ChangingViewToExtraPage;
        public event Action ChangingViewToMainPage;
        public event Action OpeningView;
        public event Action ClosingView;
        public event Action ExploreButtonDisabling;

        protected TModel buildingModel;
        protected RewardsManager rewardsManager;
        private bool isDisposed;

        public CityBuildingViewModel(TModel buildingModel, RewardsManager rewardsManager) : this()
        {
            this.buildingModel = buildingModel;
            this.rewardsManager = rewardsManager;
        }

        public CityBuildingViewModel()
        {

        }

        public void OnOpenViewButtonClicked()
        {
            OpeningView?.Invoke();
        }

        public virtual void OnExitButtonClicked()
        {
            DataPersistenceManager.GetInstance().SaveGame();
            InvokeOnClosingView();
        }

        public void OnBackButtonClicked()
        {
            ChangingViewToMainPage?.Invoke();
        }

        public void OnExploreButtonClicked(StoryRewardData exploreStory)
        {
            rewardsManager.ReceiveStoryReward(exploreStory, shouldLockCursorAfterStoryEnded: false);
            ExploreButtonDisabling?.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    ChangingViewToExtraPage = null;
                    ChangingViewToMainPage = null;
                    OpeningView = null;
                    ClosingView = null;
                    ExploreButtonDisabling = null;

                    buildingModel = null;
                    rewardsManager = null;
                }

                isDisposed = true;
            }

            base.Dispose(disposing);
        }

        protected void InvokeChangingViewToExtraPage()
        {
            ChangingViewToExtraPage?.Invoke();
        }

        protected void InvokeOnClosingView()
        {
            ClosingView?.Invoke();
        }
    }
}
