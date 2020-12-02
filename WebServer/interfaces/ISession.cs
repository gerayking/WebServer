namespace WebServer.interfaces
{
    /**
     * @description: Session 类，管理Session，存储在内存中
     * @date:  2020年11月11日10:57:18
     * @author: gerayking
     * **/
    public interface ISession
    {
        object this[string name] { get; set; }
        void Remove(string name);
    }
}