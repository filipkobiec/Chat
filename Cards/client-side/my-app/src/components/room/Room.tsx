import Chat from "./Chat/Chat";
import MessageModel from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";

function Room({ room, messages, sendMessage, closeRoomConnection, kickUserFromRoom, user} : {room : RoomModel, messages : MessageModel[], sendMessage : any, closeRoomConnection : any, kickUserFromRoom: any, user: UserModel}) {
    const history = useHistory();

    if (user.name === ''){
        return(
            <div>
                <p>Please visit main page in order to select or create room</p>
                <Button variant='danger' onClick={() => {
                        closeRoomConnection();
                        history.push("/");
                    }}
                    >Get back to main menu</Button>
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
                                {user.isAdmin &&
                                    <Button onClick={() => {
                                        kickUserFromRoom(room.id, p.id);
                                    }}>Kick</Button>
                                }
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