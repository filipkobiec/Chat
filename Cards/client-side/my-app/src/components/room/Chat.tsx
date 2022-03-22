import MessageContainer from "../MessageContainer";
import Message from "../../models/Message"
import SendMessageForm from "../SendMessageForm"

function Chat({ messages, sendMessage} : {messages : Message[], sendMessage : any}) {
    return(
        <div>
            <div className="chat">
                <MessageContainer messages={messages}></MessageContainer>
                <SendMessageForm sendMessage={sendMessage}/>
            </div>
        </div>
    )
}

export default Chat