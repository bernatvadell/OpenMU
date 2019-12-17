﻿// <copyright file="QuestStartAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Quest;

    /// <summary>
    /// A player action which implements the starting of a quest.
    /// </summary>
    public class QuestStartAction
    {
        /// <summary>
        /// Tries to start the quest of the given group and number for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="group">The group.</param>
        /// <param name="number">The number.</param>
        public void StartQuest(Player player, short group, short number)
        {
            var quest = player.OpenedNpc?.Definition.Quests.FirstOrDefault(q => q.Group == group && q.Number == number);
            if (quest == null)
            {
                // todo log
                return;
            }

            if (quest.MinimumCharacterLevel > player.Level || quest.MaximumCharacterLevel < player.Level)
            {
                // todo log
                return;
            }

            var questState = player.SelectedCharacter.QuestStates.FirstOrDefault(q => q.Group == group);
            if (questState == null)
            {
                questState = player.PersistenceContext.CreateNew<CharacterQuestState>();
                questState.Group = group;
                player.SelectedCharacter.QuestStates.Add(questState);
            }

            if (questState.ActiveQuest != null)
            {
                // todo log
                return;
            }

            if (questState.LastFinishedQuest == quest && !quest.Repeatable)
            {
                // todo log
                return;
            }

            questState.Clear(player.PersistenceContext);
            questState.ActiveQuest = quest;
            player.ViewPlugIns.GetPlugIn<IQuestStartedPlugIn>()?.QuestStarted(quest);
        }
    }
}