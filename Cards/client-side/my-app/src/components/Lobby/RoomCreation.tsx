import { useState } from "react";
import { Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../../models/UserModel";
import styles from './RoomCreation.module.scss'; 

function RoomCreation({ createRoom, user}: {createRoom : any, user : UserModel}) {
    const [room, setRoom] = useState('');
    const history = useHistory();
    return (
         <div className={styles.roomCreation}>
            <h2>Create Room</h2>
            <Form 
                onSubmit={e => {
                    e.preventDefault();
                    createRoom(user, room);
                    history.push(`room/${room}`)
                }}
            >
                <Form.Group>
                    <Form.Control placeholder="name" onChange={e => user.name = e.target.value} />
                    <Form.Control placeholder="room" onChange={e => setRoom(e.target.value)} />
                </Form.Group>
                <button className="custom-default-btn" type="submit" disabled={!user || !room }>Join</button>
            </Form>
         </div>

    );
}
export default RoomCreation;