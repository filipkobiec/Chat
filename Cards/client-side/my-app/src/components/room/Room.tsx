import Chat from "./Chat/Chat";
import MessageModel from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";

function Room({ room, messages, sendMessage, closeRoomConnection, user} : {room : RoomModel, messages : MessageModel[], sendMessage : any, closeRoomConnection : any, user: UserModel}) {
    const history = useHistory();

    if (room.userModels.length === 0){
        return(
            <div>
                disconnected
                <Button variant='danger' onClick={() => {
                        closeRoomConnection();
                        history.push("/");
                    }}
                    >Leave Room</Button>
            </div>
        )
    }
    else{
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
                    {room.userModels.map((p, index) => {
                        if (p.isAdmin) {
                            return (
                                <div key={index}>
                                <div>
                                    {p.name}
                                </div>
                                <div>
                                    Admin
                                </div>
                            </div>
                            )
                        }
                        return (
                            <div key={index}>
                            <div>
                                {p.name}
                            </div>
                        </div>
                        )
                    }
                    )}
                </div>
                <Chat user={user} messages = {messages} sendMessage = {sendMessage}></Chat>
            </div>
        )
    }
    
}

export default Room 