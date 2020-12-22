namespace Tauron.Application.CommonUI.ModelMessages
{
    public sealed class InitParentViewModel
    {
        public InitParentViewModel(IViewModel model)
        {
            Model = model;
        }

        public IViewModel Model { get; }
    }
}