import {useState } from "react";
import { Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";
import styles from './JoinRoom.module.scss'; 

type JoinRoomFunction = (user: UserModel, room: string) => void;

function JoinRoom({joinRoom, rooms} : {joinRoom : JoinRoomFunction, rooms: RoomModel[]}) {
    
    const [user, setUser] = useState('');
    const history = useHistory();
    let roomName : string;
    let roomId  : string;
    return (
        <div className={styles.joinRoom}>
            <h2>Join Room</h2>
            <Form className="hubs-container"
                onSubmit={e => {
                    e.preventDefault();
                    const userModel = new UserModel();
                    userModel.name = user;

                    joinRoom(userModel, roomId);
                    history.push(`room/${roomName}`)
                }}
            >
                <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
                {
                    rooms.map((element, index) => 
                            (<div key={index}>
                                <h3>{element.roomName}</h3>
                                <button className="custom-default-btn"  type="submit" disabled={!user} onClick={() => {roomName=element.roomName; roomId = element.id}}>Join</button>
                            </div>
                        )
                )}
            </Form> 
        </div>
   
    );
}

export default JoinRoom;