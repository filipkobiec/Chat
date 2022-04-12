import React, { useEffect, useState } from "react";
import { Button, Form } from "react-bootstrap"
import { useHistory } from "react-router-dom"
import UserModel from "../../models/UserModel";
import RoomModel from "../../models/RoomModel";

function JoinRoom({joinRoom, rooms} : {joinRoom : any, rooms: RoomModel[]}) {
    
    const [user, setUser] = useState('');
    const history = useHistory();
    let roomName : string;
    let roomId  : string;
    return (
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
                            <Button variant="success" type="submit" disabled={!user} onClick={() => {roomName=element.roomName; roomId = element.id}}>Join</Button>
                        </div>
                    )
            )}
        </Form>
    );
}

export default JoinRoom;