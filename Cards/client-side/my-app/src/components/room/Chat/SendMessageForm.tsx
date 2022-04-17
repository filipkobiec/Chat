import { type } from "os";
import { useState } from "react";
import UserModel from "../../../models/UserModel";
import styles from "./SendMessageForm.module.scss"

export type SendMessageFunction = (user: UserModel, message: string) => void;

function SendMessageForm({user, sendMessage } : {user : UserModel, sendMessage : SendMessageFunction}) {
    const [message, setMessage] = useState('');
    return (
        <form className={styles.form} onSubmit={e => {
                     e.preventDefault();
                     sendMessage(user, message);
                     setMessage('');
                }}>
            <input className={styles.input} onChange={e => setMessage(e.target.value)}
                value={message} placeholder="message..."/>
            <button className={`custom-default-btn  ${styles.btn}`} type="submit" disabled={!message}>Send</button>

      </form>
    )
}

export default SendMessageForm