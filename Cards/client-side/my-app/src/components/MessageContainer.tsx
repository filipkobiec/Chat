import Message from "../models/Message"

function MessageContainer({ messages } : {messages:Message[]}){
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