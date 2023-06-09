﻿using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using QuizerServer.HelperInterfaces;
using QuizerServer.Models;

namespace Quizer.Models;

public class Groups : BaseModel
{
    public string? Name { get; set; }
    public ICollection<Users>? Users { get; set; }
    public ICollection<Tasks>? Tasks { get; set; }
}

public class GroupsServices : IServices<Groups>
{
    public ApplicationContext? db { get; set; }

    async public Task AddEntity(Dictionary<string, object> value)
    {
        try
        {
            await db!.Groups.AddAsync(new Groups()
            {
                Name = value["name"].ToString(),
            });

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    async public Task<List<Groups>> EntityLIst()
    {
        return await db!.Groups.ToListAsync();
    }

    public IEnumerable<Groups> GetEntity()
    {
        return db!.Groups.Select(x => x);
    }

    async public Task<Groups?> GetEntity(Dictionary<string, object> value)
    {
        if (value.ContainsKey("name")) return await db!.Groups.FirstOrDefaultAsync(predicate: x => x.Name == value["name"].ToString());

        throw new Exception("Не передано название группы");
    }
}