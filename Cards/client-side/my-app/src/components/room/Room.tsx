import Chat from "./Chat";
import Message from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import {useParams} from 'react-router-dom';
import UserModel from "../../models/UserModel";
import CardGame from "./CardGame"
import WhiteCard from "../cards/WhiteCard";

function Room({ messages, sendMessage, closeRoomConnection, players, player, startGame } : {messages : Message[], sendMessage : any, closeRoomConnection : any, players: UserModel[], player: UserModel, startGame: any}) {
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
            <div>
                <Button variant='danger' onClick={() => {
                    startGame(id, player);
                }}
                >Start Game</Button>
            </div>
            {player.cards.map((c, index) => {
                console.log(c)
                return(
                    <WhiteCard text={c.text}/>
                )
            })}
            <CardGame player={player} players={players}></CardGame>
            <Chat messages = {messages} sendMessage = {sendMessage}></Chat>
        </div>
    )
}

export default Room 