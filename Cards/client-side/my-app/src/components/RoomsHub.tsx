import React, { useEffect, useState } from "react";
import { Button, Form } from "react-bootstrap"
import axios from "axios"
import { useHistory } from "react-router-dom"

function RoomsHub({joinRoom} : {joinRoom : any}) {
    
    const [user, setUser] = useState('');
    const [rooms, setRooms] = useState<string[]>([]);
    const history = useHistory();
    let room : string;
        useEffect(() => {
            axios.get('https://localhost:44319/hubs')
            .then(response => {
                setRooms(response.data);
                console.log(rooms)
            })
        }, []);
  
    return (
        <Form className="hubs-container"
            onSubmit={e => {
                e.preventDefault();
                joinRoom(user, room);
                history.push(`room/${room}`)
            }}
        >
            
            <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
            {console.log(rooms)}
            {
                rooms.map(element => 
                        (<div>
                            <h3>{element}</h3>
                            <Button variant="success" type="submit" disabled={!user} onClick={() => room=element}>Join</Button>
                        </div>
                        )
            )}
        </Form>
    );
}

export default RoomsHub;