import ChatBox from "./ChatBox";
import MessageModel from "../../../models/Message"
import SendMessageForm, { SendMessageFunction } from "../../room/Chat/SendMessageForm"
import UserModel from "../../../models/UserModel";
import styles from "./Chat.module.scss"

function Chat({user, messages, sendMessage} : {user : UserModel, messages : MessageModel[], sendMessage : SendMessageFunction}) {
    return(
        <div className={styles.chat}>
            <ChatBox user={user} messages={messages}></ChatBox>
            <SendMessageForm user={user} sendMessage={sendMessage}/>
        </div>
    )
}

export default Chat