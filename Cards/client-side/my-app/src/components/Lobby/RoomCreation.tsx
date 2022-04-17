import { useState } from "react";
import { Form } from "react-bootstrap"
import UserModel from "../../models/UserModel";
import styles from './RoomCreation.module.scss'; 

type CreateRoomFunction = (user: UserModel, room: string) => void;

function RoomCreation({ createRoom, user}: {createRoom : CreateRoomFunction, user : UserModel}) {
    const [room, setRoom] = useState('');
    const [username, setUsername] = useState('');
    return (
         <div className={styles.roomCreation}>
            <h2>Create Room</h2>
            <Form 
                onSubmit={e => {
                    e.preventDefault();
                    user.name = username;
                    createRoom(user, room);
                }}
            >
                <Form.Group>
                    <Form.Control placeholder="name" onChange={e => setUsername(e.target.value)} />
                    <Form.Control placeholder="room" onChange={e => setRoom(e.target.value)} />
                </Form.Group>
                <button className="custom-default-btn" type="submit" disabled={!user || !room }>Join</button>
            </Form>
         </div>

    );
}
export default RoomCreation;