import UserModel from "../../models/UserModel";

function CardGame({players, player} : {players: UserModel[], player: UserModel}) {
    if (player.isPlayerTurn){
        return <AdminPlayBoard/>
    }
    else{
        return <PlayBoard/>
    }
}

function AdminPlayBoard(){
    return(
        <div>
            <h2>Tura gracza</h2>
        </div>
    )
}
function PlayBoard(){
    return(
        <div>
            <h2>Nie twoja tura</h2>
        </div>
    )
}



export default CardGame