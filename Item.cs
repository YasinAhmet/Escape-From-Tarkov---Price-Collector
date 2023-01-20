namespace TarkovFLeaM; 

public class Item {
    public string name;
    public string price;
    public string id;

    public Item(string name, string id, string price) {
        this.id = id;
        this.name = name;
        this.price = price;
    }
}