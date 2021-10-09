import MessageContainer from "./MessageContainer";
import Chat from "./Chat";
import Message from "../models/Message"
import SendMessageForm from "./SendMessageForm"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import React from "react";
import UserModel from "../models/UserModel";

function Room({ messages, sendMessage, closeConnection, players } : {messages : Message[], sendMessage : any, closeConnection : any, players: UserModel[]}) {
    const history = useHistory();
    return(
        <div>
            <div>
                {players.map((p, index) => 
                    <div key={index}>
                        <div>
                            {p.name}
                        </div>
                        <div>
                            {p.isAdmin}
                        </div>
                        <div>
                            {p.points}
                        </div>
                    </div>
                )}
            </div>
            <Chat messages = {messages} sendMessage = {sendMessage} closeConnection = {closeConnection}></Chat>
        </div>
    )
}

export default Room