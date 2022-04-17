import styles from "./UsersList.module.scss"
import UserModel from "../../../models/UserModel";
import RoomModel from "../../../models/RoomModel";

export type KickUserFromRoomFunction = (roomId: string, userId: string) => void;

function UsersList({user, room, kickUserFromRoom} : {user: UserModel, room : RoomModel, kickUserFromRoom: KickUserFromRoomFunction}) {
    return(
        <div className={styles.listContainer}>
            {room.userModels.map((p, index) => {
                if (p.isAdmin) {
                    return (
                        <div key={index} className={styles.user}>
                            <div>
                                {p.name}
                            </div>
                            <div>
                                (Admin)
                            </div>
                        </div>
                    )
                }
                return (
                    <div key={index} className={styles.user}>
                        <div>
                            {p.name}
                        </div>
                        {user.isAdmin &&
                            <button className={`custom-default-btn ${styles.btn}`} onClick={() => {
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