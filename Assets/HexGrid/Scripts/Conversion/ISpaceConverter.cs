
namespace Hex
{
    interface ISpaceConverter<FromSpace, ToSpace>
    {
        ToSpace convertTo(FromSpace position);
        FromSpace convertFrom(ToSpace position);
    }
}
