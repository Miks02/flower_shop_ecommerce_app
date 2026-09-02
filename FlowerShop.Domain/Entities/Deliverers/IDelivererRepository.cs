namespace FlowerShop.Domain.Entities.Deliverers;

public interface IDelivererRepository
{
    void Add(Deliverer deliverer);
    void Update(Deliverer deliverer);
    void Remove(Deliverer deliverer);
}