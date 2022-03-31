import { Button } from 'react-bootstrap';
import CardModel from '../../models/CardModel';
import UserModel from '../../models/UserModel';
import styles from './ChosenCard.module.scss'; 


function chosenCard({player, card, handleWinnerCard} : {player: UserModel, card : CardModel, handleWinnerCard: any}) {
    const disabledClass = "disabled";
    let liClasses = [styles.card];
    if (!(player.isPlayerTurn && player.isPlayerCardChar)){
        liClasses.push(disabledClass)
    }
    return(
        <Button className={liClasses.join(' ')} onClick={() => {handleWinnerCard(card, player.roomId)}}>
            <h2>{card.text}</h2>
        </Button>
    )
}

export default chosenCard