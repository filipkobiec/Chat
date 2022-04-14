import MessageContainer from "../MessageContainer";
import Message from "../../models/Message"
import SendMessageForm from "../SendMessageForm"
import UserModel from "../../models/UserModel";

function Chat({user, messages, sendMessage} : {user : UserModel, messages : Message[], sendMessage : any}) {
    return(
        <div>
            <div className="chat">
                <MessageContainer user={user} messages={messages}></MessageContainer>
                <SendMessageForm user={user} sendMessage={sendMessage}/>
            </div>
        </div>
    )
}

export default Chat