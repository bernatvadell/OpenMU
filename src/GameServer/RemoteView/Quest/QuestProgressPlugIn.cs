﻿// <copyright file="QuestProgressPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IQuestProgressPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Progress", "The default implementation of the IQuestProgressPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("5D2B7F90-FEFA-4889-B339-D64512471613")]
    public class QuestProgressPlugIn : IQuestProgressPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestProgressPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public QuestProgressPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc/>
        public void ShowQuestProgress(QuestDefinition quest, bool wasProgressionRequested)
        {
            using var writer = this.player.Connection.StartSafeWrite(QuestProgress.HeaderType, QuestProgress.Length);
            _ = new QuestProgress(writer.Span)
            {
                QuestGroup = (ushort)quest.Group,
            };

            var questState = this.player.SelectedCharacter.QuestStates.FirstOrDefault(q => q.Group == quest.Group);

            // to write the quest state into the message, we can use the same logic as for the QuestState. The messages are equal in their content.
            QuestState progress = writer.Span;
            progress.AssignActiveQuestData(questState, this.player);
            writer.Commit();
        }
    }
}