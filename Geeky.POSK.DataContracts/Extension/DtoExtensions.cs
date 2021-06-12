using AutoMapper;
using CommonServiceLocator;
using Geeky.POSK.DataContracts.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geeky.POSK.DataContracts.Extensions
{
  public static class DtoExtensions<T>
  {
    static IMapper _mapper;
    static DtoExtensions()
    {
      _mapper = ServiceLocator.Current.GetInstance<IMapper>();
    }
  }
  public static class DtoExtensions
  {
    static IMapper _mapper;
    static DtoExtensions()
    {
      _mapper = ServiceLocator.Current.GetInstance<IMapper>();
    }
    public static T MapTo<T>(this BaseDto dto)
    {
      return _mapper.Map<T>(dto);
    }
    public static T MapTo<T>(this BaseDto dto, IMapper mapper = null)
    {
      mapper = mapper ?? _mapper;
      return mapper.Map<T>(dto);
    }
    public static IEnumerable<T> MapTo<T>(this IEnumerable<BaseDto> dto, IMapper mapper = null)
    {
      mapper = mapper ?? _mapper;
      return mapper.Map<IEnumerable<T>>(dto);
    }
    public static IEnumerable<T> MapTo<T>(this BaseDto[] dto, IMapper mapper = null)
    {
      mapper = mapper ?? _mapper;
      return mapper.Map<IEnumerable<T>>(dto);
    }
    public static IEnumerable<U> MapTo<U>(this IEnumerable dto)
    {
      return _mapper.Map<IEnumerable<U>>(dto);
    }
    public static void MapTo<T,U>(this T dto, out U target)
    {
      target = _mapper.Map<U>(dto);
    }
    
  }



}