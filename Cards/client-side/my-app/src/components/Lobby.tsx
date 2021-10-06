import { useState } from "react";
import { Button, Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../models/UserModel";

function Lobby({ joinRoom }: any) {
    const [user, setUser] = useState('');
    const [room, setRoom] = useState('');
    const history = useHistory();
    return (
        <Form className="lobby"
            onSubmit={e => {
                e.preventDefault();
                const userModel : UserModel = {
                    name : user,
                    isAdmin : true,
                    points : 0
                }
                joinRoom(userModel, room);
                history.push(`room/${room}`)
            }}
        >
            <Form.Group>
                <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
                <Form.Control placeholder="room" onChange={e => setRoom(e.target.value)} />
            </Form.Group>
            <Button variant="success" type="submit" disabled={!user || !room }>Join</Button>
        </Form>
    );
}
export default Lobby;