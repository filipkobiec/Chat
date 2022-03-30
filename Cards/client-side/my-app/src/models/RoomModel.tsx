import CardModel from "./CardModel";
import UserModel from "./UserModel";

export default class RoomModel {
    public roomName: string = '';
    public userModels: UserModel[] = [];
    public blackCard: CardModel | undefined;
    public blackCards: CardModel[] = [];
    public chosenCards: CardModel[] = [];
  }