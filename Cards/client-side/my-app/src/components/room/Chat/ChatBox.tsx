import MessageModel from "../../../models/Message"
import UserModel from "../../../models/UserModel";
import Message from "./Message";
import styles from './ChatBox.module.scss'; 


function ChatBox({ user, messages } : {user: UserModel, messages:MessageModel[]}){
    return (
        <div className={styles.chatBox}>
            {messages.map((m, index) => 
                <Message key={index} user={user} message={m}></Message>
            )}
        </div>
    )
}

export default ChatBox;