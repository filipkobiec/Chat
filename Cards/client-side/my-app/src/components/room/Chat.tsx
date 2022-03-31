import MessageContainer from "../MessageContainer";
import Message from "../../models/Message"
import SendMessageForm from "../SendMessageForm"
import UserModel from "../../models/UserModel";

function Chat({player, messages, sendMessage} : {player : UserModel, messages : Message[], sendMessage : any}) {
    return(
        <div>
            <div className="chat">
                <MessageContainer messages={messages}></MessageContainer>
                <SendMessageForm player={player} sendMessage={sendMessage}/>
            </div>
        </div>
    )
}

export default Chat