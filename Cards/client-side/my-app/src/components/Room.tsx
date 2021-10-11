import MessageContainer from "./MessageContainer";
import Chat from "./Chat";
import Message from "../models/Message"
import SendMessageForm from "./SendMessageForm"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import React from "react";
import UserModel from "../models/UserModel";

function Room({ messages, sendMessage, closeRoomConnection, players } : {messages : Message[], sendMessage : any, closeRoomConnection : any, players: UserModel[]}) {
    const history = useHistory();
    return(
        <div>
            <div className="leave-room">
                <Button variant='danger' onClick={() => {
                    closeRoomConnection();
                    history.push("/");
                }}
                >Leave Room</Button>
            </div>
            <div>
                {console.log({players})}
                {players.map((p, index) => {
                    if (p.isAdmin) {
                        return (
                            <div key={index}>
                            <div>
                                {p.name}
                            </div>
                            <div>
                                Admin
                            </div>
                            <div>
                                {p.points}
                            </div>
                        </div>
                        )
                    }
                    return (
                        <div key={index}>
                        <div>
                            {p.name}
                        </div>
                        <div>
                            {p.points}
                        </div>
                    </div>
                    )
                }
                )}
            </div>
            <Chat messages = {messages} sendMessage = {sendMessage}></Chat>
        </div>
    )
}

export default Room