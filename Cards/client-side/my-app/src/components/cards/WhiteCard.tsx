import CardModel from '../../models/CardModel';
import UserModel from '../../models/UserModel';
import styles from './WhiteCard.module.scss'; 


function WhiteCard({player, card, sendCardPlayerChose} : {player: UserModel, card : CardModel, sendCardPlayerChose: any}) {
    return(
        <div className={styles.card} onClick={() => {sendCardPlayerChose(player, card)}}>
            <h2>{card.text}</h2>
        </div>
    )
}

export default WhiteCard