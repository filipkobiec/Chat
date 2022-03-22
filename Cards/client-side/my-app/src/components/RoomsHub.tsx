import React, { useEffect, useState } from "react";
import { Button, Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../models/UserModel";
import RoomModel from "../models/RoomModel";

function RoomsHub({joinRoom, rooms} : {joinRoom : any, rooms: RoomModel[]}) {
    
    const [user, setUser] = useState('');
    const history = useHistory();
    let room : string;
    return (
        <Form className="hubs-container"
            onSubmit={e => {
                e.preventDefault();
                const userModel = new UserModel();
                userModel.name = user;

                joinRoom(userModel, room);
                history.push(`room/${room}`)
            }}
        >
            
            <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
            {
                rooms.map((element, index) => 
                        (<div key={index}>
                            <h3>{element.roomName}</h3>
                            <Button variant="success" type="submit" disabled={!user} onClick={() => room=element.roomName}>Join</Button>
                        </div>
                    )
            )}
        </Form>
    );
}

export default RoomsHub;