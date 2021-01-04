﻿using System.Threading.Tasks;
using TextAdventures.Engine.Actors;
using TextAdventures.Engine.Systems;

namespace TextAdventures.Engine.Modules.Text
{
    public sealed class TextCoordinator : CoordinatorProcess, IConsumeEvent<UpdateTextLayerEvent>
    {
        private readonly Task<TextLayerComponent> _textData;
        private TextLayerComponent TextData => _textData.Result;

        public TextCoordinator() 
            => _textData = GetGlobalComponent<TextLayerComponent>();

        public void Process(UpdateTextLayerEvent evt)
        {
            var (name, textData) = evt;
            EmitEvents(TextData, new TextLayerEvent(name, textData));
            SentUpdate();
        }

        protected override void LoadingCompled(LoadingCompled obj) 
            => SentUpdate();

        private void SentUpdate()
        {
            string title = string.Empty;
            string main = string.Empty;
            string detail = string.Empty;

            foreach (var textData in TextData.TextData)
            {
                if (string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(textData.Title))
                    title = textData.Title;
                if (string.IsNullOrWhiteSpace(main) && !string.IsNullOrWhiteSpace(textData.MainDescription))
                    main = textData.MainDescription;
                if (string.IsNullOrWhiteSpace(detail) && !string.IsNullOrWhiteSpace(textData.Detail))
                    detail = textData.Detail;
            }

            EmitEvents(new TextEvent(title, main, detail));
        }
    }
}