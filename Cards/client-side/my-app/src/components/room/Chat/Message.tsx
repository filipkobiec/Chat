import MessageModel from "../../../models/Message";
import UserModel from "../../../models/UserModel";
import './Message.scss'; 

function Message({ user, message } : { user:UserModel, message:MessageModel }){

    let className : string;
    switch (message.user) {
        case user.name:
            className = "user";
            break;
        case "MyChat Bot":
            className = "bot"
            break;
        default:
            className = "other-user"
            break;
    }

    return (
    <div className={`${className}-message-container message-container`}>
        <div className="message">
            {className !== "user" && 
                <div className="from-user">{message.user}</div>
            }
            <div className={`${className}-message-text message-text`}>{message.message}</div>
        </div>
    </div>
    )
}

export default Message;