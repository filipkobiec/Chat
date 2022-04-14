import ChatBox from "./ChatBox";
import MessageModel from "../../../models/Message"
import SendMessageForm from "../../room/Chat/SendMessageForm"
import UserModel from "../../../models/UserModel";

function Chat({user, messages, sendMessage} : {user : UserModel, messages : MessageModel[], sendMessage : any}) {
    return(
        <div>
            <div className="chat">
                <ChatBox user={user} messages={messages}></ChatBox>
                <SendMessageForm user={user} sendMessage={sendMessage}/>
            </div>
        </div>
    )
}

export default Chat