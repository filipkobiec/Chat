import { Button } from 'react-bootstrap';
import CardModel from '../../models/CardModel';
import UserModel from '../../models/UserModel';
import styles from './WhiteCard.module.scss'; 


function WhiteCard({player, card, sendCardPlayerChose} : {player: UserModel, card : CardModel, sendCardPlayerChose: any}) {
    const disabledClass = "disabled";
    let liClasses = [styles.card];
    if (!player.isPlayerTurn || player.isPlayerCardChar){
        liClasses.push(disabledClass)
    }
    return(
        <Button className={liClasses.join(' ')} onClick={() => {sendCardPlayerChose(player, card)}}>
            <h2>{card.text}</h2>
        </Button>
    )
}

export default WhiteCard