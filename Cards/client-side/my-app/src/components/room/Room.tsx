import Chat from "./Chat/Chat";
import MessageModel from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";
import UsersList, { KickUserFromRoomFunction } from "./Chat/UsersList";
import styles from "./Room.module.scss"
import { SendMessageFunction } from "./Chat/SendMessageForm";

type CloseRoomConnectionFunction = () => void;

function Room({ room, messages, sendMessage, closeRoomConnection, kickUserFromRoom, user} : {room : RoomModel, messages : MessageModel[], sendMessage : SendMessageFunction, closeRoomConnection : CloseRoomConnectionFunction, kickUserFromRoom: KickUserFromRoomFunction, user: UserModel}) {
    const history = useHistory();

    if (user.name === ''){
        return(
            <div>
                <p style={{color: "white"}}>Please visit main page in order to select or create room</p>
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
                <div className={styles.chatContainer}>
                    <UsersList user={user} room={room} kickUserFromRoom={kickUserFromRoom}/>
                    <Chat user={user} messages = {messages} sendMessage = {sendMessage}></Chat>
                </div>
            </div>

        )
    }
    
}

export default Room