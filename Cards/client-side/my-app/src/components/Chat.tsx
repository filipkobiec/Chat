import MessageContainer from "./MessageContainer";
import Message from "../models/Message"
import SendMessageForm from "./SendMessageForm"
import {Button} from "react-bootstrap"
import {useHistory} from "react-router-dom"

function Chat({ messages, sendMessage, closeConnection } : {messages : Message[], sendMessage : any, closeConnection : any}) {
    const history = useHistory();
    return(
        <div>
            <div className="leave-room">
                <Button variant='danger' onClick={() => {
                    closeConnection();
                    history.push("/");
                }}
                >Leave Room</Button>
            </div>
            <div className="chat">
                <MessageContainer messages={messages}></MessageContainer>
                <SendMessageForm sendMessage={sendMessage}/>
            </div>
        </div>
    )
}

export default Chat