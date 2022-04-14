import MessageModel from "../../../models/Message"
import UserModel from "../../../models/UserModel";
import Message from "./Message";
import styles from './ChatBox.module.scss'; 
import { useEffect, useRef } from "react";


function ChatBox({ user, messages } : {user: UserModel, messages:MessageModel[]}){

    const messagesEndRef = useRef<HTMLDivElement>(null)

    const scrollToBottom = () => {
      messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
    }
  
    useEffect(() => {
      scrollToBottom()
    }, [messages]);
    
    return (
        <div className={styles.chatBox}>
            {messages.map((m, index) => 
                <Message key={index} user={user} message={m}></Message>
            )}
            <div ref={messagesEndRef}></div>
        </div>
    )
}

export default ChatBox;