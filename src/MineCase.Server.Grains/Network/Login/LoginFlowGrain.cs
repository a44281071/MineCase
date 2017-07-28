﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using MineCase.Protocol.Login;
using System.Threading.Tasks;
using MineCase.Server.Player;

namespace MineCase.Server.Network.Login
{
    class LoginFlowGrain : Grain, ILoginFlow
    {
        private bool _useAuthentication = false;

        public async Task DispatchPacket(LoginStart packet)
        {
            if (_useAuthentication)
                throw new NotImplementedException();
            else
            {
                var uuid = await GrainFactory.GetGrain<INonAuthenticatedPlayer>(packet.Name).GetUUID();
                await SendLoginSuccess(packet.Name, uuid);
            }
        }

        private async Task SendLoginSuccess(string userName, Guid uuid)
        {
            var sink = GrainFactory.GetGrain<IClientboundPaketSink>(this.GetPrimaryKey());
            await GrainFactory.GetGrain<IPacketRouter>(this.GetPrimaryKey()).Play();
            await sink.SendPacket(new LoginSuccess
            {
                Username = userName,
                UUID = uuid.ToString()
            });
        }
    }
}