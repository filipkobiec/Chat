import { useState } from "react";
import { Button, Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../../models/UserModel";

function RoomCreation({ createRoom, user}: {createRoom : any, user : UserModel}) {
    const [room, setRoom] = useState('');
    const history = useHistory();
    return (
        <Form className="lobby"
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
            <Button variant="success" type="submit" disabled={!user || !room }>Join</Button>
        </Form>
    );
}
export default RoomCreation;