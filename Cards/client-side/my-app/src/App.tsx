import { useEffect, useState } from "react";
import './scss/App.scss';
import * as signalR from "@microsoft/signalr"
import 'bootstrap/dist/css/bootstrap.min.css'
import Lobby from "./components/Lobby";
import Room from "./components/room/Room";
import { LogLevel } from "@microsoft/signalr";
import Message from "./models/Message";
import UserModel from "./models/UserModel";
import RoomModel from "./models/RoomModel"
import RoomsHub from "./components/RoomsHub"
import {
    BrowserRouter as Router,
    Route,
    Switch,
} from "react-router-dom";
import CardModel from "./models/CardModel";



function App() {
    const [connection, setConnection] = useState<signalR.HubConnection>();
    const [messages, setMessages] = useState<Message[]>([]);
    const [players, setPlayers] = useState<UserModel[]>([]);
    const [player, setPlayer] = useState<UserModel>(new UserModel());
    const [rooms, setRooms] = useState<RoomModel[]>([]);
    
    useEffect(() => {
        makeConnection();
    }, [])
    
    const makeConnection = async () => {
        try {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("https://localhost:44319/chat")
                .configureLogging(LogLevel.Information)
                .build();

            connection.on("ReceiveMessage", (user: string, message: string) => {
                setMessages(messages => [...messages, {user, message}]);
            });

            connection.on("UpdatePlayers", (players: UserModel[]) => {
                setPlayers(players)
            })

            connection.on("ReceiveRooms", (rooms: RoomModel[]) => {
                setRooms(rooms);
            })

            connection.on("SetPlayer", (user: UserModel) => {
                setPlayer(user);
            })

            connection.onclose(e => {
                setConnection(undefined);
                setMessages([]);
            })

            await connection.start();
            await connection.invoke("GetRooms")
            setConnection(connection);
        }
        catch (e) {
            console.log(e);
        }
    }
    const joinRoom = async (user: UserModel, room : string) => {
        setMessages([])
        try {
            await connection?.invoke("JoinRoom", { user, room })
        } catch (e) {
            console.log(e);
        }
    }

    const sendMessage = async (message : string) => {
      try {
        await connection?.invoke("SendMessage", message);
      } catch (e) {
        console.log(e)
      }
    };

    const sendCardPlayerChose = async (player: UserModel, userCard: CardModel) => {
        try {
            await connection?.invoke("GetChosenCard",  player, userCard )
        } catch (e) {
            console.log(e);
        }
    }

    const CloseRoomConnection = async () => {
        try {
            await connection?.invoke("CloseRoomConnection");
        } catch (e) {
            console.log(e)
        }
    }

    
    const StartGame = async (room: string, player: UserModel) => {
        try {
            await connection?.invoke("StartGame", room, player);
        } catch (e) {
            console.log(e)
        }
    }


    return (
        <div className="app">
            <h2>MyChat</h2>
            <hr className="line" />
            <Router>
                <Switch>
                    <Route exact path="/">
                        <RoomsHub joinRoom={joinRoom} rooms={rooms}/>
                        <Lobby joinRoom={joinRoom}/>
                    </Route>
                    <Route path="/room/:id">
                        <Room messages={messages} sendMessage={sendMessage} 
                        closeRoomConnection={CloseRoomConnection} players={players} player={player} startGame={StartGame} sendCardPlayerChose={sendCardPlayerChose}/>
                    </Route>
                </Switch>
            </Router>
        </div>
    );
};
export default App;
