import Chat from "./Chat";
import Message from "../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import UserModel from "../models/UserModel";
import CardGame from "./CardGame"

function Room({ messages, sendMessage, closeRoomConnection, players, player } : {messages : Message[], sendMessage : any, closeRoomConnection : any, players: UserModel[], player: UserModel}) {
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
                                {p.isPlayerTurn}
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
            <CardGame player={player} players={players}></CardGame>
            <Chat messages = {messages} sendMessage = {sendMessage}></Chat>
        </div>
    )
}

export default Room 