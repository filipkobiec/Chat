import { useEffect, useState } from "react";
import './App.scss';
import * as signalR from "@microsoft/signalr"
import 'bootstrap/dist/css/bootstrap.min.css'
import RoomCreation from "./components/Lobby/RoomCreation";
import Room from "./components/room/Room";
import { LogLevel } from "@microsoft/signalr";
import MessageModel from "./models/Message";
import UserModel from "./models/UserModel";
import RoomModel from "./models/RoomModel"
import JoinRoom from "./components/Lobby/JoinRoom"
import {
    BrowserRouter as Router,
    Route,
    Switch,
} from "react-router-dom";
import LoadingSpinner from "./components/LoadingSpinner/LoadingSpinner";
import NoServerConnection from "./components/Errors/NoServerConnection/NoServerConnection";



function App() {
    const [connection, setConnection] = useState<signalR.HubConnection>();
    const [messages, setMessages] = useState<MessageModel[]>([]);
    const [user, setUser] = useState<UserModel>(new UserModel());
    const [room, setRoom] = useState<RoomModel>(new RoomModel());
    const [rooms, setRooms] = useState<RoomModel[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isErrorWithConnection, setIsErrorWithConnection] = useState(false);
    
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

            connection.on("RedirectUserToMainMenu", () => {
                window.location.href="/"
            })

            connection.on("SetUser", (user: UserModel) => {
                setUser(user);
            })

            connection.onclose(e => {
                setConnection(undefined);
                setMessages([]);
                setIsErrorWithConnection(true);
            })

            await connection.start();
            await connection.invoke("GetRooms")
            setIsLoading(false);
            setConnection(connection);
        }
        catch (e) {
            if (e instanceof TypeError) {
                setIsLoading(false);
                setIsErrorWithConnection(true);
            }
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

    const kickUserFromRoom = async ( roomId : string, userId : string) => {
        try {
            await connection?.invoke("KickUserFromRoom", roomId, userId);
        } catch (e) {
            console.log(e)
        }
    }

    const closeRoomConnection = async () => {
        try {
            await connection?.invoke("CloseRoomConnection");
        } catch (e) {
            console.log(e)
        }
    }


    return (
        <div className="app">
            {isLoading && <LoadingSpinner />}
            {isErrorWithConnection && <NoServerConnection />}
            <h2>Chat</h2>
            <hr className="line" />
            <Router>
                <Switch>
                    <Route exact path="/">
                        <RoomCreation createRoom={createRoom} user={user}/>
                        {rooms.length !== 0 &&
                            <JoinRoom joinRoom={joinRoom} rooms={rooms}/>
                        }
                    </Route>
                    <Route path="/room/:id">
                        <Room room={room} messages={messages} sendMessage={sendMessage} 
                        closeRoomConnection={closeRoomConnection} kickUserFromRoom={kickUserFromRoom} user={user} />
                    </Route>
                </Switch>
            </Router>
        </div>
    );
};
export default App;
