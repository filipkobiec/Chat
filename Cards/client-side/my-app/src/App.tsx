import { useEffect, useState } from "react";
import './App.css';
import * as signalR from "@microsoft/signalr"
import 'bootstrap/dist/css/bootstrap.min.css'
import Lobby from "./components/Lobby";
import Room from "./components/Room";
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



function App() {
    const [connection, setConnection] = useState<signalR.HubConnection>();
    const [messages, setMessages] = useState<Message[]>([]);
    const [players, setPlayers] = useState<UserModel[]>([]);
    const [player, setPlayer] = useState<UserModel>({ name : "default",
        isAdmin : false,
        points : 0,
        isPlayerTurn : false});
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

    const CloseRoomConnection = async () => {
        try {
            await connection?.invoke("CloseRoomConnection");
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
                        closeRoomConnection={CloseRoomConnection} players={players} player={player}/>
                    </Route>
                </Switch>
            </Router>
        </div>
    );
};
export default App;
