using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Geeky.POSK.WPF.Core.Base
{
  [DataContract]
  public abstract class BindableBaseViewModel : INotifyPropertyChanged
  {
    protected virtual void SetProperty<T>(ref T member, T val,
           [CallerMemberName] string propertyName = null)
    {
      if (object.Equals(member, val)) return;

      member = val;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged = delegate { };
  }

  [DataContract]
  public sealed class IdName : BindableBaseViewModel
  {
    public IdName(int id, string name)
    {
      Id = id;
      Name = name;
    }

    private int _id;
    private string _name;
    [DataMember] public int Id { get { return _id; } set { SetProperty(ref _id, value); } }
    [DataMember] public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

  }
}
