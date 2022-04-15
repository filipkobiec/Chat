import styles from "./UsersList.module.scss"
import UserModel from "../../../models/UserModel";
import { Button } from "react-bootstrap";
import RoomModel from "../../../models/RoomModel";

function UsersList({user, room, kickUserFromRoom} : {user: UserModel, room : RoomModel, kickUserFromRoom: any}) {
    return(
        <div>
            {room.userModels.map((p, index) => {
                if (p.isAdmin) {
                    return (
                        <div key={index}>
                            <div>
                                {p.name}
                            </div>
                            <div>
                                Admin
                            </div>
                        </div>
                    )
                }
                return (
                    <div key={index}>
                        <div>
                            {p.name}
                        </div>
                        {user.isAdmin &&
                            <button className="custom-default-btn" onClick={() => {
                                kickUserFromRoom(room.id, p.id);
                            }}>Kick</button>
                        }
                    </div>
                )
            }
            )}
        </div>
    )
}

export default UsersList