import Chat from "./Chat";
import Message from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import {useParams} from 'react-router-dom';
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";

function Room({ room, messages, sendMessage, closeRoomConnection, player} : {room : RoomModel, messages : Message[], sendMessage : any, closeRoomConnection : any, player: UserModel}) {
    const history = useHistory();
    const { id } = useParams() as {id: string};
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
            <Chat player={player} messages = {messages} sendMessage = {sendMessage}></Chat>
        </div>
    )
}

export default Room 