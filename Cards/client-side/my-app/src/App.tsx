import { useEffect, useState } from "react";
import './scss/App.scss';
import * as signalR from "@microsoft/signalr"
import 'bootstrap/dist/css/bootstrap.min.css'
import RoomCreation from "./components/Lobby/RoomCreation";
import Room from "./components/room/Room";
import { LogLevel } from "@microsoft/signalr";
import Message from "./models/Message";
import UserModel from "./models/UserModel";
import RoomModel from "./models/RoomModel"
import JoinRoom from "./components/Lobby/JoinRoom"
import {
    BrowserRouter as Router,
    Route,
    Switch,
    useHistory,
} from "react-router-dom";



function App() {
    const [connection, setConnection] = useState<signalR.HubConnection>();
    const [messages, setMessages] = useState<Message[]>([]);
    const [player, setPlayer] = useState<UserModel>(new UserModel());
    const [room, setRoom] = useState<RoomModel>(new RoomModel());
    const [rooms, setRooms] = useState<RoomModel[]>([]);
    const history = useHistory();
    
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

            connection.on("UpdateRoom", (room: RoomModel) => {
                setRoom(room)
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

    const createRoom = async (user: UserModel, roomName: string) => {
        setMessages([])
        try {
            await connection?.invoke("CreateRoom",  user, roomName )
        } catch (e) {
            console.log(e);
        }
    }
    
    const joinRoom = async (user: UserModel, roomId : string) => {
        setMessages([])
        try {
            await connection?.invoke("JoinRoom",  user, roomId )
        } catch (e) {
            console.log(e);
        }
    }

    const sendMessage = async (user: UserModel ,message : string) => {
      try {
        await connection?.invoke("SendMessage", user, message);
      } catch (e) {
        console.log(e)
      }
    };


    const closeRoomConnection = async () => {
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
                        {rooms.length != 0 &&
                        <JoinRoom joinRoom={joinRoom} rooms={rooms}/>
                        }
                        <RoomCreation createRoom={createRoom} user={player}/>
                    </Route>
                    <Route path="/room/:id">
                        <Room room={room} messages={messages} sendMessage={sendMessage} 
                        closeRoomConnection={closeRoomConnection} player={player} />
                    </Route>
                </Switch>
            </Router>
        </div>
    );
};
export default App;
