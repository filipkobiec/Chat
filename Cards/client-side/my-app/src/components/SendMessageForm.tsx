import { useState } from "react";
import { FormControl, InputGroup, Button, Form } from "react-bootstrap"
import UserModel from "../models/UserModel";

function SendMessageForm({player, sendMessage } : {player : UserModel, sendMessage : any}) {
    const [message, setMessage] = useState('');
    return (
        <Form
            onSubmit={e => {
                e.preventDefault();
                sendMessage(player, message);
                setMessage('');
            }}>
            <InputGroup>
                <FormControl placeholder="message..."
                    onChange={e => setMessage(e.target.value)}
                    value={message}/>
                <InputGroup.Append>
                    <Button variant="primary" type="submit"
                        disabled={!message}
                    >Send</Button>
                </InputGroup.Append>
            </InputGroup>
        </Form>
    )
}

export default SendMessageForm