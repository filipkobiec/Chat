import Chat from "./Chat";
import Message from "../../models/Message"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"
import {useParams} from 'react-router-dom';
import UserModel from "../../models/UserModel";
import CardGame from "./CardGame"
import WhiteCard from "../cards/WhiteCard";
import RoomModel from "../../models/RoomModel";

function Room({ room, messages, sendMessage, closeRoomConnection, player, startGame, sendCardPlayerChose} : {room : RoomModel, messages : Message[], sendMessage : any, closeRoomConnection : any, player: UserModel, startGame: any, sendCardPlayerChose: any}) {
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
            {player.isAdmin &&
                <div>
                    <Button variant='danger' onClick={() => {
                        startGame(player);
                    }}
                    >Start Game</Button>
                </div>
            }

            {room.blackCard &&
                <div>
                    {room.blackCard.text}
                </div>
            }
            <div>
                {room.chosenCards.map((c, index) => {
                    return(
                        <WhiteCard player={player} card={c} sendCardPlayerChose={sendCardPlayerChose}  key={index}/>
                    )
                })}
            </div>
            {player.cards.map((c, index) => {
                return(
                    <WhiteCard player={player} card={c} sendCardPlayerChose={sendCardPlayerChose}  key={index}/>
                )
            })}
            <CardGame player={player} players={room.userModels}></CardGame>
            <Chat player={player} messages = {messages} sendMessage = {sendMessage}></Chat>
        </div>
    )
}

export default Room 