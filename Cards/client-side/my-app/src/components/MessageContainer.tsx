import Message from "../models/Message"
import UserModel from "../models/UserModel";

function MessageContainer({ user, messages } : {user: UserModel, messages:Message[]}){
    return (
        <div className="message-container">
            {messages.map((m, index) => 
                <div key={index} className="user-message">
                    <div className="message bg-primary">{m.message}</div>
                    <div className="from-user">{m.user}</div>
                </div>
            )}
        </div>
    )
}

export default MessageContainer;