import MessageModel from "../../../models/Message"
import UserModel from "../../../models/UserModel";
import Message from "./Message";
import styles from './ChatBox.module.scss'; 
import { useEffect, useRef } from "react";
import { Scrollbar } from "react-scrollbars-custom";



function ChatBox({ user, messages } : {user: UserModel, messages:MessageModel[]}){

    const messagesEndRef = useRef<HTMLDivElement>(null)

    const scrollToBottom = () => {
      messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
    }
  
    useEffect(() => {
      scrollToBottom()
    }, [messages]);
    
    return (
      <Scrollbar
       style={{ height: 400}}
       scrollerProps={{
        renderer: props => {
          const { elementRef, ...restProps } = props;
          return <span {...restProps} ref={elementRef} className={styles.chatBox} />;
        }
      }}
      trackYProps={{
        renderer: props => {
          const { elementRef, ...restProps } = props;
          return <span {...restProps} ref={elementRef} className={styles.trackY} />;
        }
      }}
      >
        {messages.map((m, index) => 
            <Message key={index} user={user} message={m}></Message>
        )}
        <div ref={messagesEndRef}></div>
      </Scrollbar>
    )
}

export default ChatBox;