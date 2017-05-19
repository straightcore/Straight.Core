using System.Runtime.InteropServices;

namespace Straight.Core.Sample.RealEstateAgency.Contracts
{
    public interface IModelConverter
    {
        TDto ToDto<TDto>(object model)
            where TDto : class;

        TModel ToModel<TModel>(object dto) 
            where TModel : class;
    }

    public interface IModelConverter<TModel, TDto>
    {
        TDto ToDto(TModel model);

        TModel ToModel(TDto dto);
    }
}
